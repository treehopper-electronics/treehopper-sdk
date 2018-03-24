try:
    from setuptools import setup
except ImportError:
    from distutils.core import setup

setup(
    name='treehopper',
    version='1.11.4',
    packages=['treehopper.api','treehopper.libraries', 'treehopper.utils'],
    license='MIT license',
    url='https://treehopper.io',
    author='Jay Carlson/Treehopper Electronics',
    author_email='jay@treehopper.io',
    description='Treehopper USB boards connect the physical world to your computer, smartphone, or tablet.',
    long_description=open('README.txt').read(),
    install_requires=[
        'pyusb'
    ]
)
